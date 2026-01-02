import requests
import pandas as pd
import numpy as np
from datetime import datetime, timedelta
import plotly.express as px
import plotly.graph_objects as go
from plotly.subplots import make_subplots
import json
from collections import defaultdict
import warnings
warnings.filterwarnings('ignore')

class MorphRunnerRealtimeAnalytics:
    def __init__(self, firebase_url="https://morphrunneranalytics3107-default-rtdb.firebaseio.com/"):
        """
        Initialize Firebase Realtime Database connection
        
        Args:
            firebase_url (str): Firebase Realtime Database URL
        """
        self.firebase_url = firebase_url.rstrip('/') + '/'
        
    def fetch_all_users_data(self):
        """
        Fetch all user data from Firebase Realtime Database
        
        Returns:
            dict: Complete user data structure
        """
        try:
            response = requests.get(f"{self.firebase_url}users.json")
            if response.status_code == 200:
                data = response.json()
                if data:
                    print(f"Fetched data for {len(data)} users")
                    return data
                else:
                    print("No data found in Firebase")
                    return {}
            else:
                print(f"Error fetching data: {response.status_code}")
                return {}
        except Exception as e:
            print(f"Error connecting to Firebase: {e}")
            return {}
    
    def process_user_data(self, raw_data):
        """
        Process raw Firebase data into structured DataFrames
        
        Args:
            raw_data (dict): Raw data from Firebase
            
        Returns:
            tuple: (user_summary_df, level_details_df, mismatch_positions_df, health_completion_df)
        """
        user_summaries = []
        level_details = []
        mismatch_positions = []
        health_completions = []  # NEW: Health completion data
        
        for user_id, user_data in raw_data.items():
            if not isinstance(user_data, dict):
                continue
            
            # Extract username and registration time from Firebase
            username = user_data.get('username', f'Player_{user_id[:8]}')
            registration_time = user_data.get('registration_time', 'Unknown')
                
            # Extract death counts per level
            death_data = {}
            match_stats = user_data.get('match_stats', {})
            completion_stats = user_data.get('completion_stats', {})  # NEW: Get completion stats
            
            # Process death counts (level_name_death_times)
            for key, value in user_data.items():
                if key.endswith('_death_times') and isinstance(value, (int, float)):
                    level_name = key.replace('_death_times', '')
                    death_data[level_name] = value
            
            # NEW: Process health completion data
            for level_name, completion_data in completion_stats.items():
                if isinstance(completion_data, dict):
                    completion_count = completion_data.get('completion_count', 0)
                    health_values = completion_data.get('health_remaining_values', {})
                    
                    # Extract all health values for this level
                    if isinstance(health_values, dict):
                        for health_id, health_value in health_values.items():
                            if isinstance(health_value, (int, float)):
                                # Convert to percentage (assuming max health is 100)
                                health_percentage = (float(health_value) / 100.0) * 100
                                health_completions.append({
                                    'user_id': user_id,
                                    'username': username,
                                    'level_name': level_name,
                                    'health_remaining': float(health_value),
                                    'health_percentage': health_percentage,
                                    'completion_count': completion_count
                                })
            
            # Process match statistics
            for level_name, level_stats in match_stats.items():
                if isinstance(level_stats, dict):
                    match_count = level_stats.get('obstacle_match_count', 0)
                    mismatch_count = level_stats.get('obstacle_mismatch_count', 0)
                    mismatch_positions_data = level_stats.get('obstacle_mismatch_positions', {})
                    
                    deaths = death_data.get(level_name, 0)
                    
                    # Calculate accuracy
                    total_attempts = match_count + mismatch_count
                    accuracy = match_count / total_attempts if total_attempts > 0 else 0
                    
                    level_detail = {
                        'user_id': user_id,
                        'username': username,
                        'level_name': level_name,
                        'deaths': deaths,
                        'correct_matches': match_count,
                        'mismatches': mismatch_count,
                        'total_attempts': total_attempts,
                        'accuracy': accuracy
                    }
                    level_details.append(level_detail)
                    
                    # Process mismatch positions
                    if isinstance(mismatch_positions_data, dict):
                        for pos_id, y_position in mismatch_positions_data.items():
                            if isinstance(y_position, (int, float)):
                                mismatch_positions.append({
                                    'user_id': user_id,
                                    'username': username,
                                    'level_name': level_name,
                                    'y_position': float(y_position),
                                    'position_id': pos_id
                                })
            
            # Create user summary
            user_levels = [detail for detail in level_details if detail['user_id'] == user_id]
            if user_levels:
                total_deaths = sum(detail['deaths'] for detail in user_levels)
                total_matches = sum(detail['correct_matches'] for detail in user_levels)
                total_mismatches = sum(detail['mismatches'] for detail in user_levels)
                total_attempts = total_matches + total_mismatches
                overall_accuracy = total_matches / total_attempts if total_attempts > 0 else 0
                
                # Calculate score: Correct matches give +10 points, mismatches give -10 points
                score = total_matches * 10 + total_mismatches * (-10)  # Same as: total_matches * 10 - total_mismatches * 10
                
                user_summaries.append({
                    'user_id': user_id,
                    'username': username,
                    'registration_time': registration_time,
                    'total_deaths': total_deaths,
                    'total_correct_matches': total_matches,
                    'total_mismatches': total_mismatches,
                    'total_attempts': total_attempts,
                    'overall_accuracy': overall_accuracy,
                    'levels_played': len(user_levels),
                    'score': score
                })
        
        return (pd.DataFrame(user_summaries), 
                pd.DataFrame(level_details), 
                pd.DataFrame(mismatch_positions),
                pd.DataFrame(health_completions))  # NEW: Return health data
    
    def create_complete_dashboard(self, user_df, level_df, mismatch_df, health_df):
        """Create a focused dashboard with key metrics, health graph, leaderboard and summary"""
        
        # Create extended dashboard layout (3 rows x 3 columns)
        fig = make_subplots(
            rows=3, cols=3,
            subplot_titles=(
                'Level 1: Mismatch Positions', 'Level 2: Mismatch Positions', 'Level 3: Mismatch Positions',
                'Total Deaths per Level', 'Obstacle Match Accuracy', 'Health Remaining at Completion',
                '🏆 Top Players Leaderboard', '', '📊 Summary Statistics'
            ),
            specs=[
                [{"type": "histogram"}, {"type": "histogram"}, {"type": "histogram"}],
                [{"type": "bar"}, {"type": "bar"}, {"type": "scatter"}],  # Changed last to scatter for health graph
                [{"type": "bar", "colspan": 2}, None, {"type": "table"}]  # Leaderboard spans 2 cols, summary in col 3
            ],
            vertical_spacing=0.12,
            horizontal_spacing=0.10
        )
        
        # Row 1: Mismatch Position Histograms for each level
        levels = ['Level1', 'Level2', 'Level3']
        colors = ['#9C27B0', '#E91E63', '#FF5722']
        
        for i, level in enumerate(levels):
            if not mismatch_df.empty:
                level_mismatches = mismatch_df[mismatch_df['level_name'] == level]
                if not level_mismatches.empty:
                    fig.add_trace(
                        go.Histogram(
                            x=level_mismatches['y_position'],
                            nbinsx=12,
                            name=f'{level} Mismatches',
                            marker_color=colors[i],
                            showlegend=False
                        ),
                        row=1, col=i+1
                    )
                else:
                    fig.add_trace(
                        go.Histogram(x=[], name='No Data', marker_color=colors[i], showlegend=False),
                        row=1, col=i+1
                    )
            else:
                fig.add_trace(
                    go.Histogram(x=[], name='No Data', marker_color=colors[i], showlegend=False),
                    row=1, col=i+1
                )
        
        # Row 2, Col 1: Total Deaths per Level
        if not level_df.empty:
            deaths_by_level = level_df.groupby('level_name')['deaths'].sum().sort_index()
            fig.add_trace(
                go.Bar(
                    x=deaths_by_level.index,
                    y=deaths_by_level.values,
                    name='Total Deaths per Level',
                    marker_color='#FF6B6B',
                    showlegend=False
                ),
                row=2, col=1
            )
        else:
            fig.add_trace(
                go.Bar(x=[], y=[], name='No Data', marker_color='#FF6B6B', showlegend=False),
                row=2, col=1
            )
        
        # Row 2, Col 2: Obstacle Match Accuracy (Stacked Bar Chart)
        if not level_df.empty:
            # Calculate accuracy data by level
            accuracy_data = level_df.groupby('level_name').agg({
                'correct_matches': 'sum',
                'mismatches': 'sum',
                'total_attempts': 'sum'
            }).sort_index()
            
            # Calculate percentages for stacked bar
            correct_percentages = []
            incorrect_percentages = []
            level_names = []
            
            for level in accuracy_data.index:
                total = accuracy_data.loc[level, 'total_attempts']
                if total > 0:
                    correct_pct = (accuracy_data.loc[level, 'correct_matches'] / total) * 100
                    incorrect_pct = (accuracy_data.loc[level, 'mismatches'] / total) * 100
                else:
                    correct_pct = 0
                    incorrect_pct = 0
                
                correct_percentages.append(correct_pct)
                incorrect_percentages.append(incorrect_pct)
                level_names.append(level)
            
            # Add correct matches (green) - bottom of stack
            fig.add_trace(
                go.Bar(
                    x=level_names,
                    y=correct_percentages,
                    name='Correct Matches',
                    marker_color='#4CAF50',  # Green
                    text=[f'{pct:.1f}%' for pct in correct_percentages],
                    textposition='inside',
                    showlegend=False
                ),
                row=2, col=2
            )
            
            # Add incorrect matches (red) - top of stack
            fig.add_trace(
                go.Bar(
                    x=level_names,
                    y=incorrect_percentages,
                    name='Incorrect Matches',
                    marker_color='#F44336',  # Red
                    text=[f'{pct:.1f}%' for pct in incorrect_percentages],
                    textposition='inside',
                    base=correct_percentages,  # Stack on top of correct matches
                    showlegend=False
                ),
                row=2, col=2
            )
        else:
            fig.add_trace(
                go.Bar(x=[], y=[], name='No Data', marker_color='#CCCCCC', showlegend=False),
                row=2, col=2
            )
        
        # Row 2, Col 3: Health Remaining at Level Completion
        if not health_df.empty:
            # Calculate average health percentage by level
            health_by_level = health_df.groupby('level_name')['health_percentage'].mean().sort_index()
            
            fig.add_trace(
                go.Scatter(
                    x=[1, 2, 3, 4],  # Level numbers
                    y=[health_by_level.get('Level1', 0), 
                       health_by_level.get('Level2', 0),
                       health_by_level.get('Level3', 0), 
                       health_by_level.get('Level4', 0)],
                    mode='lines+markers',
                    name='Average Health Remaining (%)',
                    line=dict(color='#2196F3', width=3),
                    marker=dict(color='#2196F3', size=8),
                    showlegend=False
                ),
                row=2, col=3
            )
            
            # Add ideal range lines (40-70% as shown in your mockup)
            fig.add_hline(y=70, line_dash="dash", line_color="green", 
                         annotation_text="Ideal Range (Max)", row=2, col=3)
            fig.add_hline(y=40, line_dash="dash", line_color="green", 
                         annotation_text="Ideal Range (Min)", row=2, col=3)
        else:
            # Empty health graph
            fig.add_trace(
                go.Scatter(x=[1, 2, 3, 4], y=[0, 0, 0, 0], mode='lines+markers',
                          name='No Health Data', line=dict(color='#CCCCCC'), showlegend=False),
                row=2, col=3
            )
        
        # Row 3, Col 1-2: Leaderboard (spans 2 columns)
        if not user_df.empty:
            # Sort by score and take top 10
            top_players = user_df.nlargest(10, 'score')
            
            fig.add_trace(
                go.Bar(
                    x=top_players['username'],
                    y=top_players['score'],
                    name='Player Scores',
                    marker_color='#4CAF50',
                    text=[f"Score: {score}<br>Accuracy: {acc:.1%}<br>Mismatches: {mismatches}" 
                          for score, acc, mismatches in zip(top_players['score'], 
                                                       top_players['overall_accuracy'], 
                                                       top_players['total_mismatches'])],
                    textposition='outside',
                    showlegend=False
                ),
                row=3, col=1
            )
        else:
            fig.add_trace(
                go.Bar(x=['No Players Yet'], y=[0], name='No Data', marker_color='#4CAF50', showlegend=False),
                row=3, col=1
            )
        
        # Row 3, Col 3: Summary Statistics Table
        if not user_df.empty and not level_df.empty:
            top_player = user_df.loc[user_df['score'].idxmax(), 'username'] if not user_df.empty else 'N/A'
            
            # Add health statistics if available
            avg_health_completion = health_df['health_percentage'].mean() if not health_df.empty else 0
            
            summary_data = [
                ['Total Players', len(user_df)],
                ['🏆 Top Player', top_player],
                ['Total Deaths', level_df['deaths'].sum()],
                ['Overall Accuracy', f"{level_df['accuracy'].mean():.1%}"],
                ['Avg Accuracy', f"{user_df['overall_accuracy'].mean():.1%}"],
                ['Avg Health at Completion', f"{avg_health_completion:.1f}%"],
                ['Hardest Level', level_df.groupby('level_name')['deaths'].sum().idxmax() if not level_df.empty else 'N/A'],
                ['Highest Accuracy Level', level_df.groupby('level_name')['accuracy'].mean().idxmax() if not level_df.empty else 'N/A']
            ]
            
            fig.add_trace(
                go.Table(
                    header=dict(values=['Metric', 'Value'], 
                               fill_color='#E3F2FD',
                               font=dict(size=12)),
                    cells=dict(values=[[row[0] for row in summary_data], 
                                     [row[1] for row in summary_data]], 
                             fill_color='#F5F5F5',
                             font=dict(size=11))
                ),
                row=3, col=3
            )
        else:
            # Empty table for no data
            fig.add_trace(
                go.Table(
                    header=dict(values=['Metric', 'Value'], 
                               fill_color='#E3F2FD',
                               font=dict(size=12)),
                    cells=dict(values=[['No Data Available'], ['Play the game!']], 
                             fill_color='#F5F5F5',
                             font=dict(size=11))
                ),
                row=3, col=3
            )
        
        # Update layout
        fig.update_layout(
            height=1200,
            showlegend=False,
            title_text="🎮 Morph Runner: Complete Analytics Dashboard",
            title_x=0.5,
            title_font_size=18,
            font=dict(size=12)
        )
        
        # Add proper axis labels
        # Row 1 (Histograms)
        fig.update_xaxes(title_text="Y-Position", row=1, col=1)
        fig.update_yaxes(title_text="Mismatch Count", row=1, col=1)
        
        fig.update_xaxes(title_text="Y-Position", row=1, col=2)
        fig.update_yaxes(title_text="Mismatch Count", row=1, col=2)
        
        fig.update_xaxes(title_text="Y-Position", row=1, col=3)
        fig.update_yaxes(title_text="Mismatch Count", row=1, col=3)
        
        # Row 2
        fig.update_xaxes(title_text="Level", row=2, col=1)
        fig.update_yaxes(title_text="Total Deaths", row=2, col=1)
        
        fig.update_xaxes(title_text="Level", row=2, col=2)
        fig.update_yaxes(title_text="Match Accuracy (%)", row=2, col=2)
        fig.update_yaxes(range=[0, 100], row=2, col=2)  # Set Y-axis to 0-100%
        
        # NEW: Health graph
        fig.update_xaxes(title_text="Level Number", row=2, col=3)
        fig.update_yaxes(title_text="Average Health Remaining (%)", row=2, col=3)
        fig.update_yaxes(range=[0, 100], row=2, col=3)  # Set Y-axis to 0-100%
        
        # Row 3 (Leaderboard)
        fig.update_xaxes(title_text="Player Username", row=3, col=1)
        fig.update_yaxes(title_text="Score", row=3, col=1)
        
        return fig
    
    def print_summary_stats(self, user_df, level_df, mismatch_df, health_df):
        """Print comprehensive summary statistics including health data"""
        print("=" * 60)
        print("🎮 MORPH RUNNER ANALYTICS SUMMARY")
        print("=" * 60)
        
        if not user_df.empty:
            print(f"📊 PLAYER OVERVIEW:")
            print(f"   Total Players: {len(user_df)}")
            print(f"   Average Deaths per Player: {user_df['total_deaths'].mean():.2f}")
            print(f"   Average Accuracy: {user_df['overall_accuracy'].mean():.2%}")
            print(f"   Average Attempts per Player: {user_df['total_attempts'].mean():.1f}")
            print(f"   Average Levels Played: {user_df['levels_played'].mean():.1f}")
            print()
        
        if not level_df.empty:
            print(f"🎮 LEVEL ANALYSIS:")
            level_summary = level_df.groupby('level_name').agg({
                'deaths': 'sum',
                'accuracy': 'mean',
                'mismatches': 'sum',
                'correct_matches': 'sum',
                'total_attempts': 'sum'
            }).round(2)
            
            print(f"   Total Levels: {len(level_summary)}")
            if not level_summary.empty:
                print(f"   Hardest Level (most deaths): {level_summary['deaths'].idxmax()} ({level_summary['deaths'].max():.0f} total deaths)")
                print(f"   Easiest Level (least deaths): {level_summary['deaths'].idxmin()} ({level_summary['deaths'].min():.0f} total deaths)")
                print(f"   Highest Accuracy Level: {level_summary['accuracy'].idxmax()} ({level_summary['accuracy'].max():.1%} accuracy)")
                print(f"   Lowest Accuracy Level: {level_summary['accuracy'].idxmin()} ({level_summary['accuracy'].min():.1%} accuracy)")
            print()
        
        # NEW: Health completion analysis
        if not health_df.empty:
            print(f"❤️  HEALTH COMPLETION ANALYSIS:")
            print(f"   Total Level Completions: {len(health_df)}")
            print(f"   Average Health at Completion: {health_df['health_percentage'].mean():.1f}%")
            print(f"   Best Health at Completion: {health_df['health_percentage'].max():.1f}%")
            print(f"   Worst Health at Completion: {health_df['health_percentage'].min():.1f}%")
            
            # Health by level analysis
            health_by_level = health_df.groupby('level_name')['health_percentage'].agg(['mean', 'count'])
            for level in health_by_level.index:
                avg_health = health_by_level.loc[level, 'mean']
                completions = health_by_level.loc[level, 'count']
                print(f"   {level}: {avg_health:.1f}% avg health ({completions} completions)")
            print()
        
        if not mismatch_df.empty:
            print(f"🎯 ACCURACY & MISMATCH ANALYSIS:")
            print(f"   Total Mismatches Logged: {len(mismatch_df)}")
            print(f"   Average Y-Position of Mismatches: {mismatch_df['y_position'].mean():.2f}")
            if not mismatch_df['y_position'].mode().empty:
                print(f"   Most Common Mismatch Y-Position: {mismatch_df['y_position'].mode().iloc[0]:.2f}")
            
            # Find problematic areas per level
            for level in mismatch_df['level_name'].unique():
                level_data = mismatch_df[mismatch_df['level_name'] == level]
                print(f"   {level}: {len(level_data)} mismatches, avg Y={level_data['y_position'].mean():.1f}")
            print()
        
        # Print leaderboard to console
        if not user_df.empty:
            print("🏆 LEADERBOARD (Top 10):")
            top_10 = user_df.nlargest(10, 'score')
            for i, (_, player) in enumerate(top_10.iterrows(), 1):
                medal = "🥇" if i == 1 else "🥈" if i == 2 else "🥉" if i == 3 else f"{i:2d}."
                print(f"   {medal} {player['username']:<15} | Score: {player['score']:6.0f} | Accuracy: {player['overall_accuracy']:6.1%} | Mismatches: {player['total_mismatches']:3.0f}")
            print()
    
    def generate_full_report(self):
        """Generate complete analytics report with health data"""
        print("🔄 Fetching data from Firebase Realtime Database...")
        raw_data = self.fetch_all_users_data()
        
        if not raw_data:
            print("❌ No real player data found in Firebase yet!")
            print("🎮 Play your game to generate analytics data.")
            print("📝 The dashboard will be empty until players start playing.")
            
            # Create empty DataFrames
            user_df = pd.DataFrame()
            level_df = pd.DataFrame()
            mismatch_df = pd.DataFrame()
            health_df = pd.DataFrame()
        else:
            print("✅ Using REAL data from your Firebase!")
            print("⚙️  Processing data...")
            user_df, level_df, mismatch_df, health_df = self.process_user_data(raw_data)
        
        print("📈 Generating dashboard...")
        
        # Print summary statistics to console
        if not user_df.empty:
            self.print_summary_stats(user_df, level_df, mismatch_df, health_df)
        else:
            print("📊 No data to analyze yet - dashboard will show empty charts.")
            print("🏆 Leaderboard will be empty until players start playing.")
        
        # Create the complete dashboard
        dashboard_fig = self.create_complete_dashboard(user_df, level_df, mismatch_df, health_df)
        
        # Show the dashboard in browser
        dashboard_fig.show()
        
        if not raw_data:
            print("🎯 Dashboard is empty - play your game to see real analytics!")
        else:
            print("✅ Dashboard shows your REAL game data with health tracking!")
        
        return user_df, level_df, mismatch_df, health_df

# Usage example
if __name__ == "__main__":
    # Initialize analytics with your Firebase URL
    analytics = MorphRunnerRealtimeAnalytics("https://morphrunneranalytics3107-default-rtdb.firebaseio.com/")
    
    # Generate complete report with health tracking
    user_data, level_data, mismatch_data, health_data = analytics.generate_full_report()
    